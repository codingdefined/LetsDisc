import { Component, Injector, Input, OnInit } from '@angular/core';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { PostDto, PostServiceProxy, PagedResultDtoOfPostDto } from '@shared/service-proxies/service-proxies';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { finalize } from 'rxjs/operators';

@Component({
    selector: 'app-posts',
    templateUrl: './posts.component.html',
    styleUrls: ['./posts.component.css'],
    animations: [appModuleAnimation()]
})
export class PostsComponent extends PagedListingComponentBase<PostDto> implements OnInit {

    @Input() tagValue = '';
    questions: PostDto[] = [];
    itemPluralMapping = {
        'answer': {
            '=0': 'answers',
            '=1': 'answer',
            'other': 'answers'
        },
        'view': {
            '=0': '0 views',
            '=1': '1 view',
            'other': '# views'
        },
        'vote': {
            '=0': 'votes',
            '=1': 'vote',
            'other': 'votes'
        }
    };

    constructor(injector: Injector, private _postService: PostServiceProxy) {
        super(injector);
    }

    ngOnInit(): void {
    }

    protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
        this._postService.getQuestions(request.skipCount, request.maxResultCount)
            .pipe(finalize(() => {
                finishedCallback()
            }))
            .subscribe((result: PagedResultDtoOfPostDto) => {
                this.questions = result.items;
                this.showPaging(result, pageNumber);
            });
    }

    protected delete(question: PostDto): void {
        abp.message.confirm(
            "Delete question '" + question.title + "'?",
            (result: boolean) => {
                if (result) {
                    this._postService.delete(question.id)
                        .subscribe(() => {
                            abp.notify.info("Deleted Question: " + question.title);
                            this.refresh();
                        });
                }
            }
        );
    }

}
