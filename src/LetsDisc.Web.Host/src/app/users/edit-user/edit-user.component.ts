import { Component, ViewChild, Injector, Output, EventEmitter, ElementRef, OnInit } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { UserServiceProxy, UserDto, RoleDto, UserInfo, UserDetailsDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/app-component-base';
import { finalize } from 'rxjs/operators';
import { Router, ActivatedRoute } from '@angular/router';
import { AppConsts } from '@shared/AppConsts';

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
    fileToUpload: File = null;
    files: Array<any> = new Array<any>();
    url: string = '';

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
                if (this.userDetails.profileImageUrl !== '') {
                    this.url = this.createImgPath(this.userDetails.profileImageUrl);
                }
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

    onSelectFile(files: FileList) {
        if (files.length === 0)
            return;
        this.fileToUpload = files.item(0);

        const fileReader: FileReader = new FileReader();
        fileReader.readAsDataURL(this.fileToUpload);

        fileReader.onload = (event: any) => {
            this.url = event.target.result;
        };

        this.files.push({ data: this.fileToUpload, fileName: this.fileToUpload.name });

        this._userService.uploadProfilePicture(this.files[0], this.user.id)
            .subscribe((result: string) => {
                this.userDetails.profileImageUrl = result;
            });
    }

     delete() {
        this.url = null;
     }

    createImgPath = (serverPath: string) => {
        if (serverPath) {
            return AppConsts.remoteServiceBaseUrl + `/${serverPath}?timeStamp=${Date.now()}`;
        }
        return '';
    }
}