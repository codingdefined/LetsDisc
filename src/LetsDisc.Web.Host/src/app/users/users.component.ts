import { Component, Injector, ChangeDetectorRef } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import {
    UserServiceProxy,
    UserDto,
    PagedResultDtoOfUserDetailsDto,
    UserDetailsDto
} from '@shared/service-proxies/service-proxies';
import { PagedListingComponentBase, PagedRequestDto } from 'shared/paged-listing-component-base';
import { finalize } from 'rxjs/operators';
import { AppConsts } from '@shared/AppConsts';
import { Title } from '@angular/platform-browser';

@Component({
    templateUrl: './users.component.html',
    styleUrls: ['./users.component.css'],
    animations: [appModuleAnimation()]
})
export class UsersComponent extends PagedListingComponentBase<UserDto> {

    active = false;
    users: UserDetailsDto[] = [];
    letter: string;

    constructor(
        injector: Injector,
        private _userService: UserServiceProxy,
        private ref: ChangeDetectorRef,
        private titleService: Title
    ) {
        super(injector);
        this.titleService.setTitle('List of all users - LetsDisc');
    }

    protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
        this._userService.getALLUsers(request.skipCount, request.maxResultCount)
            .pipe(finalize(() => {
                 finishedCallback()
            }))
            .subscribe((result: PagedResultDtoOfUserDetailsDto) => {
                this.users = result.items;
                this.showPaging(result, pageNumber);
                this.users.forEach((user) => {
                    user.profileImageUrl = this.createImgPath(user.profileImageUrl);
                });
            });
    }

    protected delete(user: UserDto): void {
        abp.message.confirm(
            'Delete user "' + user.fullName + '"?',
            (result: boolean) => {
                if (result) {
                    this._userService.delete(user.id)
                        .subscribe(() => {
                            abp.notify.info('Deleted User: ' + user.fullName);
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

    protected getLetter(fullName: string): string {
        const nameInitials = fullName.match(/\b(\w)/g);
        if (nameInitials) {
            const nameLetters = nameInitials.slice(0, 3).join('');
            return nameLetters.toUpperCase();
        } else {
            return fullName[0];
        }
    }
}
