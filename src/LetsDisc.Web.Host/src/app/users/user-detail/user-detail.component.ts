import { Component, OnInit, Injector } from '@angular/core';
import { UserDto, UserServiceProxy, UserInfo, UserDetailsDto, PostDto, PagedResultDtoOfPostDto, PagedResultDtoOfAnswerWithQuestion, AnswerWithQuestion } from '@shared/service-proxies/service-proxies';
import { ActivatedRoute } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';
import * as moment from 'moment';
import { AppConsts } from '@shared/AppConsts';
import { PagedRequestDto, PagedResultDto } from '@shared/paged-listing-component-base';
import { finalize } from 'rxjs/operators';
import { Title } from '@angular/platform-browser';

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
    questions: PostDto[] = null;
    answers: AnswerWithQuestion[] = null;
    public pageSize: number = 10;
    public questionPageNumber: number = 1;
    public questionTotalPages: number = 1;
    public questionTotalItems: number;
    public questionIsTableLoading = false;
    public answerPageNumber: number = 1;
    public answerTotalPages: number = 1;
    public answerTotalItems: number;
    public answerIsTableLoading = false;

    itemPluralMapping = {
        'answer': {
            '=0': ' answers',
            '=1': ' answer',
            'other': ' answers'
        },
        'profileview': {
            '=0': '0 profile views',
            '=1': '1 profile view',
            'other': '# profile views'
        },
        'view': {
            '=0': '0 views',
            '=1': '1 view',
            'other': '# views'
        },
        'question': {
            '=0': ' questions',
            '=1': ' question',
            'other': ' questions'
        },
        'vote': {
            '=0': 'votes',
            '=1': 'vote',
            'other': 'votes'
        }
    };

    constructor(
        injector: Injector,
        private _userService: UserServiceProxy,
        private route: ActivatedRoute,
        private titleService: Title
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
                this.titleService.setTitle("User " + result.user.fullName + " - LetsDisc");
            });
        this.getQuestionList(1, id);
        this.getAnswersList(1, id);
    }

    createImgPath = (serverPath: string) => {
        if (serverPath) {
            return AppConsts.remoteServiceBaseUrl + `/${serverPath}?timeStamp=${Date.now()}`;
        }
        return '';
    }

    protected questionList(userId: number, request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
        this._userService.getUserQuestions(request.skipCount, request.maxResultCount, userId)
            .pipe(finalize(() => {
                finishedCallback()
            }))
            .subscribe((result: PagedResultDtoOfPostDto) => {
                this.questions = result.items;
                this.showQuestionPaging(result, pageNumber);
            });
    }

    protected answerList(userId: number, request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
        this._userService.getUserAnswers(request.skipCount, request.maxResultCount, userId)
            .pipe(finalize(() => {
                finishedCallback()
            }))
            .subscribe((result: PagedResultDtoOfAnswerWithQuestion) => {
                this.answers = result.items;
                this.showAnswerPaging(result, pageNumber);
            });
    }

    private getPagedRequest(page: number) {
        var req = new PagedRequestDto();
        req.maxResultCount = this.pageSize;
        req.skipCount = (page - 1) * this.pageSize;
        return req;
    }

    public getQuestionDataPage(page: number): void {
        this.getQuestionList(page, this.user.id);
    }

    private getQuestionList(page: number, id: number) {
        var req = this.getPagedRequest(page);
        this.questionIsTableLoading = true;
        this.questionList(id, req, page, () => {
            this.questionIsTableLoading = false;
        });
    }

    public getAnswerDataPage(page: number): void {
        this.getAnswersList(page, this.user.id);
    }

    private getAnswersList(page: number, id: number) {
        var req = this.getPagedRequest(page);

        this.answerIsTableLoading = true;
        this.answerList(id, req, page, () => {
            this.answerIsTableLoading = false;
        });
    }

    public showQuestionPaging(result: PagedResultDto, pageNumber: number): void {
        this.questionTotalPages = ((result.totalCount - (result.totalCount % this.pageSize)) / this.pageSize) + 1;

        this.questionTotalItems = result.totalCount;
        this.questionPageNumber = pageNumber;
    }

    public showAnswerPaging(result: PagedResultDto, pageNumber: number): void {
        this.answerTotalPages = ((result.totalCount - (result.totalCount % this.pageSize)) / this.pageSize) + 1;

        this.answerTotalItems = result.totalCount;
        this.answerPageNumber = pageNumber;
    }

}

