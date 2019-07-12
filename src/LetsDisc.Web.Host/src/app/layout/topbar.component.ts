import { Component, Injector, ViewEncapsulation, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { AppAuthService } from '@shared/auth/app-auth.service';
import { UserDto } from '@shared/service-proxies/service-proxies';

@Component({
    templateUrl: './topbar.component.html',
    selector: 'top-bar'
})
export class TopBarComponent extends AppComponentBase implements OnInit {

    shownLoginName: string = "";
    userId: number;

    constructor(
        injector: Injector,
        private _authService: AppAuthService
    ) {
        super(injector);
    }

    ngOnInit() {
        this.shownLoginName = this.appSession.getUserName();
        this.userId = this.appSession.userId;
    }

    logout(): void {
        this._authService.logout();
    }
}