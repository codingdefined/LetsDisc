import { Component, Injector, ElementRef, ViewChild, Inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';
import { LoginService } from './login.service';
import { accountModuleAnimation } from '@shared/animations/routerTransition';
import { AbpSessionService } from '@abp/session/abp-session.service';
import { TokenAuthServiceProxy, ExternalAuthenticateModel, ExternalAuthenticateResultModel } from '@shared/service-proxies/service-proxies';
import { DOCUMENT } from '@angular/common';

@Component({
    templateUrl: './login.component.html',
    styleUrls: [
        './login.component.less'
    ],
    animations: [accountModuleAnimation()],
    providers: []
})
export class LoginComponent extends AppComponentBase implements OnInit {

    @ViewChild('cardBody') cardBody: ElementRef;

    submitting: boolean = false;

    constructor(
        injector: Injector,
        public loginService: LoginService,
        private _router: Router,
        private _sessionService: AbpSessionService,
        private _tokenAuthService: TokenAuthServiceProxy,
        @Inject(DOCUMENT) private document: Document,
    ) {
        super(injector);
    }

    ngOnInit() {
    }

    ngAfterViewInit(): void {
        $(this.cardBody.nativeElement).find('input:first').focus();
    }

    get multiTenancySideIsTeanant(): boolean {
        return this._sessionService.tenantId > 0;
    }

    get isSelfRegistrationAllowed(): boolean {
        if (!this._sessionService.tenantId) {
            return false;
        }

        return true;
    }

    login(): void {
        this.submitting = true;
        this.loginService.authenticate(
            () => this.submitting = false
        );
    }

    socialLogin(provider: string) {
        this.loginService.externalAuthenticate();
    }
}
