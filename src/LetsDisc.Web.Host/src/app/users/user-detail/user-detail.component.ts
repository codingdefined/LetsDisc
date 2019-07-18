import { Component, OnInit, Injector } from '@angular/core';
import { UserDto, UserServiceProxy, UserInfo, UserDetailsDto } from '@shared/service-proxies/service-proxies';
import { ActivatedRoute } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';
import * as moment from 'moment';
import { AppConsts } from '@shared/AppConsts';

@Component({
    selector: 'app-user-detail',
    templateUrl: './user-detail.component.html',
    styleUrls: ['./user-detail.component.css']
})
export class UserDetailComponent extends AppComponentBase implements OnInit {

    user: UserDto = null;
    userDetails: UserDetailsDto = null;
    questionsCount: number = 0;
    answersCount: number = 0;
    id: number;
    createdTimeAgo: string;
    url: string;

    itemPluralMapping = {
        'answer': {
            '=0': ' answers',
            '=1': ' answer',
            'other': ' answers'
        },
        'view': {
            '=0': '0 profile views',
            '=1': '1 profile view',
            'other': '# profile views'
        },
        'question': {
            '=0': ' questions',
            '=1': ' question',
            'other': ' questions'
        }
    };

    constructor(
        injector: Injector,
        private _userService: UserServiceProxy,
        private route: ActivatedRoute
    ) {
        super(injector);
    }

    ngOnInit() {
        this.route.paramMap.subscribe((params) => {
            this.id = +params.get('id');
            this.getUser(this.id);
        });
    }

    private getUser(id: number) {
        this._userService.getUser(id)
            .subscribe((result: UserInfo) => {
                this.user = result.user;
                this.userDetails = result.userDetails;
                this.questionsCount = result.questionsCount;
                this.answersCount = result.answersCount;
                this.createdTimeAgo = moment(this.user.creationTime).fromNow(true);
                this.url = this.createImgPath(result.userDetails.profileImageUrl);
            });
    }

    createImgPath = (serverPath: string) => {
        if (serverPath) {
            return AppConsts.remoteServiceBaseUrl + `/${serverPath}?timeStamp=${Date.now()}`;
        }
        return '';
    }

}

