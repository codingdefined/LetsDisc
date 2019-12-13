import { Injectable } from '@angular/core';
import { AppConsts } from '@shared/AppConsts';
import { TokenAuthServiceProxy } from '@shared/service-proxies/service-proxies';
import { AuthService } from 'angularx-social-login';

@Injectable()
export class AppAuthService {

    constructor(private _tokenAuthService: TokenAuthServiceProxy, private authService: AuthService) { }


    logout(reload?: boolean): void {
        abp.auth.clearToken();
        this.authService.signOut();
        if (reload !== false) {
            location.href = AppConsts.appBaseUrl;
        };
    }
}
