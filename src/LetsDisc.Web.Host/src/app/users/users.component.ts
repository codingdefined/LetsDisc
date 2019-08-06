import { Component, Injector, ViewChild, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { UserServiceProxy, UserDto, PagedResultDtoOfUserDto, PagedResultDtoOfUserDetailsDto, UserDetailsDto } from '@shared/service-proxies/service-proxies';
import { PagedListingComponentBase, PagedRequestDto } from 'shared/paged-listing-component-base';
import { EditUserComponent } from 'app/users/edit-user/edit-user.component';
import { finalize } from 'rxjs/operators';
import { AppConsts } from '@shared/AppConsts';
import { forEach } from '@angular/router/src/utils/collection';
import { Title } from '@angular/platform-browser';

@Component({
    templateUrl: './users.component.html',
    styleUrls: ['./users.component.css'],
    animations: [appModuleAnimation()]
})
export class UsersComponent extends PagedListingComponentBase<UserDto> {

    active: boolean = false;
    users: UserDetailsDto[] = [];

    constructor(
        injector: Injector,
        private _userService: UserServiceProxy,
        private ref: ChangeDetectorRef,
        private titleService: Title
    ) {
        super(injector);
        this.titleService.setTitle("List of all users - LetsDisc");
    }

    protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
        this._userService.getALLUsers(request.maxResultCount, request.skipCount)
            .pipe(finalize(() => {
                 finishedCallback()
            }))
            .subscribe((result: PagedResultDtoOfUserDetailsDto) => {
                this.users = result.items;
                this.showPaging(result, pageNumber);
                this.users.forEach((user) => {
                    user.profileImageUrl = this.createImgPath(user.profileImageUrl)
                });
            });
    }

    protected delete(user: UserDto): void {
        abp.message.confirm(
            "Delete user '" + user.fullName + "'?",
            (result: boolean) => {
                if (result) {
                    this._userService.delete(user.id)
                        .subscribe(() => {
                            abp.notify.info("Deleted User: " + user.fullName);
                            this.refresh();
                        });
                }
            }
        );
    }

    createImgPath = (serverPath: string) => {
        if (serverPath) {
            return AppConsts.remoteServiceBaseUrl + `/${serverPath}?timeStamp=${Date.now()}`;
        }
        return '';
    }
}
