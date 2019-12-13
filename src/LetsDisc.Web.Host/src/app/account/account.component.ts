import { Component, ViewContainerRef, OnInit, ViewEncapsulation, Injector, OnDestroy } from '@angular/core';
import { LoginService } from './login.service';
import { AppComponentBase } from '@shared/app-component-base';

@Component({
    templateUrl: './account.component.html',
    styleUrls: [
        './account.component.css'
    ]
})
export class AccountComponent extends AppComponentBase {

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

    printUser(signInSuccessData) {
        this._loginService.externalAuthenticate(signInSuccessData);
        console.log(signInSuccessData);
    }

    printError(errorData) {
        console.log(errorData);
    }
}
