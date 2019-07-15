import { Component, ViewChild, Injector, Output, EventEmitter, ElementRef, OnInit } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { UserServiceProxy, UserDto, RoleDto, UserInfo, UserDetailsDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/app-component-base';
import { finalize } from 'rxjs/operators';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
    selector: 'edit-user-modal',
    templateUrl: './edit-user.component.html',
    styleUrls: ['../users.component.css']
})
export class EditUserComponent extends AppComponentBase implements OnInit  {
    
    saving: boolean = false;
    id: number; 
    user: UserDto = null; 
    userDetails: UserDetailsDto = null;
    userInfo: UserInfo = new UserInfo();

    constructor(
        injector: Injector,
        private _userService: UserServiceProxy,
        private _router: Router,
        private route: ActivatedRoute
    ) {
        super(injector);
    }

    ngOnInit() {
        this.route.paramMap.subscribe((params) => {
            this.id = +params.get('id');
            this.getNewUser(this.id);
        });
    }

    private getNewUser(id: number) {
        this._userService.getUser(id)
            .subscribe((result: UserInfo) => {
                this.user = result.user;
                this.userDetails = result.userDetails;
            });
    }

    saveUser(): void {
        this.saving = true;
        this.userInfo.user = this.user;
        this.userInfo.userDetails = this.userDetails;
        this._userService.updateUserInfo(this.userInfo)
            .pipe(finalize(() => { this.saving = false; }))
            .subscribe((result: UserInfo) => {
                this.notify.info(this.l('SavedSuccessfully'));
                this._router.navigate(['users', result.user.id, result.user.fullName.replace(' ', '-')]);
            });
    }

    cancelClick(): void {
        this._router.navigate(['users', this.user.id]);
    }
}
