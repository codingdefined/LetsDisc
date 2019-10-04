
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { TokenAuthServiceProxy, AuthenticateModel, AuthenticateResultModel, ExternalLoginProviderInfoModel, ExternalAuthenticateModel, ExternalAuthenticateResultModel } from '@shared/service-proxies/service-proxies';
import { UrlHelper } from '@shared/helpers/UrlHelper';
import { AppConsts } from '@shared/AppConsts';

import { MessageService } from '@abp/message/message.service';
import { LogService } from '@abp/log/log.service';
import { TokenService } from '@abp/auth/token.service';
import { UtilsService } from '@abp/utils/utils.service';
import { finalize } from 'rxjs/operators';
import { Observable, BehaviorSubject } from 'rxjs';
import { GoogleLoginProvider, AuthService } from 'angularx-social-login';

@Injectable()
export class LoginService {

    static readonly twoFactorRememberClientTokenName = 'TwoFactorRememberClientToken';

    authenticateModel: AuthenticateModel;
    authenticateResult: AuthenticateResultModel;

    rememberMe: boolean;

    constructor(
        private _tokenAuthService: TokenAuthServiceProxy,
        private _router: Router,
        private _utilsService: UtilsService,
        private _messageService: MessageService,
        private _tokenService: TokenService,
        private _logService: LogService,
        private authService: AuthService
    ) {
        this.clear();
    }

    authenticate(finallyCallback?: () => void): void {
        finallyCallback = finallyCallback || (() => { });

        this._tokenAuthService
            .authenticate(this.authenticateModel)
            .pipe(finalize(() => { finallyCallback() }))
            .subscribe((result: AuthenticateResultModel) => {
                this.processAuthenticateResult(result);
            });
    }

    externalAuthenticate(): void {
        this.authService.signIn(GoogleLoginProvider.PROVIDER_ID).then(user => {
            const model = new ExternalAuthenticateModel({
                authProvider: GoogleLoginProvider.PROVIDER_ID,
                providerKey: user.id,
                providerAccessCode: user.id,
                emailAddress: user.email,
                name: user.firstName,
                surname: user.lastName
            });
            this._tokenAuthService.externalAuthenticate(model)
                .subscribe((result: ExternalAuthenticateResultModel) => {
                    if (result.accessToken) {
                        this.login(result.accessToken, result.encryptedAccessToken, result.expireInSeconds, false);
                    } else {
                        this._logService.warn('Unexpected authenticateResult!');
                        this._router.navigate(['account/login']);
                    }
                })
        });
    }

    private processAuthenticateResult(authenticateResult: AuthenticateResultModel) {
        this.authenticateResult = authenticateResult;

        if (authenticateResult.accessToken) {
            this.login(authenticateResult.accessToken, authenticateResult.encryptedAccessToken, authenticateResult.expireInSeconds, this.rememberMe);

        } else {
            this._logService.warn('Unexpected authenticateResult!');
            this._router.navigate(['account/login']);
        }
    }

    private login(accessToken: string, encryptedAccessToken: string, expireInSeconds: number, rememberMe?: boolean): void {

        var tokenExpireDate = rememberMe ? (new Date(new Date().getTime() + 1000 * expireInSeconds)) : undefined;

        this._tokenService.setToken(
            accessToken,
            tokenExpireDate
        );

        this._utilsService.setCookieValue(
            AppConsts.authorization.encrptedAuthTokenName,
            encryptedAccessToken,
            tokenExpireDate,
            abp.appPath
        );

        var initialUrl = UrlHelper.initialUrl;
        if (initialUrl.indexOf('/login') > 0) {
            initialUrl = AppConsts.appBaseUrl;
        }

        location.href = initialUrl;
    }

    private clear(): void {
        this.authenticateModel = new AuthenticateModel();
        this.authenticateModel.rememberClient = false;
        this.authenticateResult = null;
        this.rememberMe = false;
    }
}