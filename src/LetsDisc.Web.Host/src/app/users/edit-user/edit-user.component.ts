import { Component, ViewChild, Injector, Output, EventEmitter, ElementRef, OnInit } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { UserServiceProxy, UserDto, RoleDto } from '@shared/service-proxies/service-proxies';
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
        this._userService.get(id)
            .subscribe((result: UserDto) => {
                this.user = result;

            });
    }

    saveUser(): void {
        this.saving = true;
        this._userService.update(this.user)
            .pipe(finalize(() => { this.saving = false; }))
            .subscribe(() => {
                this.notify.info(this.l('SavedSuccessfully'));
            });
    }

    cancelClick(): void {
        this._router.navigate(['users', this.user.id]);
    }
}
