import { Injectable } from '@angular/core';
import { AppConsts } from '@shared/AppConsts';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { BehaviorSubject, Observable } from 'rxjs';
import { tap } from 'rxjs/operators';

@Injectable()
export class AppAuthService {

    constructor(private _tokenAuthService: TokenAuthServiceProxy) { }


    logout(reload?: boolean): void {
        abp.auth.clearToken();

        if (reload !== false) {
            location.href = AppConsts.appBaseUrl;
        };
    }
}