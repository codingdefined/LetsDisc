import { Component, ViewContainerRef, OnInit, ViewEncapsulation, Injector, OnDestroy } from '@angular/core';
import { LoginService } from './login/login.service';
import { AppComponentBase } from '@shared/app-component-base';

@Component({
    templateUrl: './account.component.html',
    styleUrls: [
        './account.component.less'
    ]
})
export class AccountComponent extends AppComponentBase implements OnInit, OnDestroy {

    private viewContainerRef: ViewContainerRef;

    versionText: string;
    currentYear: number;

    public constructor(
        injector: Injector,
        private _loginService: LoginService
    ) {
        super(injector);

        this.currentYear = new Date().getFullYear();
        this.versionText = this.appSession.application.version + ' [' + this.appSession.application.releaseDate.format('YYYYDDMM') + ']';
    }

    showTenantChange(): boolean {
        return abp.multiTenancy.isEnabled;
    }

    ngOnInit(): void {
        $('body').attr('class', 'login-page');
    }

    ngOnDestroy(): void {
        $('body').removeClass('login-page');
    }
}