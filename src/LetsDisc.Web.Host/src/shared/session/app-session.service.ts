import { Injectable } from '@angular/core';
import {
    SessionServiceProxy,
    UserLoginInfoDto,
    TenantLoginInfoDto,
    ApplicationInfoDto,
    GetCurrentLoginInformationsOutput,
    TokenAuthServiceProxy
} from '@shared/service-proxies/service-proxies';
import { AbpMultiTenancyService } from '@abp/multi-tenancy/abp-multi-tenancy.service';

@Injectable()
export class AppSessionService {

    private _user: UserLoginInfoDto;
    private _tenant: TenantLoginInfoDto;
    private _application: ApplicationInfoDto;
    private userEmail: string;

    constructor(
        private _sessionService: SessionServiceProxy,
        private _abpMultiTenancyService: AbpMultiTenancyService) {
    }

    get application(): ApplicationInfoDto {
        return this._application;
    }

    get user(): UserLoginInfoDto {
        return this._user;
    }

    get userId(): number {
        return this.user ? this.user.id : null;
    }

    get tenant(): TenantLoginInfoDto {
        return this._tenant;
    }

    get tenantId(): number {
        return this.tenant ? this.tenant.id : null;
    }

    get isLoggedIn(): boolean {
        return this._user ? true : false;
    }

    getShownLoginName(): string {
        const userName = this._user ? this._user.userName : 'Guest';
        if (!this._abpMultiTenancyService.isEnabled) {
            return userName;
        }

        return (this._tenant ? this._tenant.tenancyName : '.') + '\\' + userName;
    }

    getUserName(): string {
        return (this._user ? this._user.name + ' ' + this._user.surname : 'Guest');
    }

    init(): Promise<boolean> {

        return new Promise<boolean>((resolve, reject) => {
            this._sessionService.getCurrentLoginInformations(this.userEmail).toPromise()
                .then((result: GetCurrentLoginInformationsOutput) => {
                    this._application = result.application;
                    this._user = result.user;
                    this._tenant = result.tenant;
                    resolve(true);
                });
        });
    }

    changeTenantIfNeeded(tenantId?: number): boolean {
        if (this.isCurrentTenant(tenantId)) {
            return false;
        }

        abp.multiTenancy.setTenantIdCookie(tenantId);
        location.reload();
        return true;
    }

    private isCurrentTenant(tenantId?: number) {
        if (!tenantId && this.tenant) {
            return false;
        } else if (tenantId && (!this.tenant || this.tenant.id !== tenantId)) {
            return false;
        }

        return true;
    }
}
