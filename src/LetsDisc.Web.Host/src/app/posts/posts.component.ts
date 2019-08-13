import { Component, Injector, Input, OnInit, SimpleChanges, OnChanges } from '@angular/core';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { PostDto, PostServiceProxy, PagedResultDtoOfPostDto } from '@shared/service-proxies/service-proxies';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { finalize } from 'rxjs/operators';
import { Title } from '@angular/platform-browser';
import { ActivatedRoute } from '@angular/router';

@Component({
    selector: 'app-posts',
    templateUrl: './posts.component.html',
    styleUrls: ['./posts.component.css'],
    animations: [appModuleAnimation()]
})
export class PostsComponent extends PagedListingComponentBase<PostDto>{

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
    searchString: string;
    sortByValue: string;
    defaultSortBy: string = 'newest';

    constructor(injector: Injector, private _postService: PostServiceProxy, private titleService: Title, private route: ActivatedRoute) {
        super(injector);
        this.titleService.setTitle("LetsDisc - List of all questions");
    }

    ngOnChanges(changes: SimpleChanges) {
        if (changes['tagValue']) {
            this.sortByValue = this.defaultSortBy;
            this._postService.getQuestions(this.sortByValue, 0, 10, this.tagValue)
                .subscribe((result: PagedResultDtoOfPostDto) => {
                    this.questions = result.items;
                    this.showPaging(result, 0);
                });
            this.titleService.setTitle("List of '" + this.tagValue + "' questions - LetsDisc");
        }
    }

    protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
        this.route.queryParamMap.subscribe(urlParams => this.searchString = urlParams.get('q'));
        this._postService.getQuestions(this.sortByValue, request.skipCount, request.maxResultCount, this.tagValue)
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

    public sortBy(value: string) {
        this.sortByValue = value;
        this._postService.getQuestions(value, 0, 10, this.tagValue)
            .subscribe((result: PagedResultDtoOfPostDto) => {
                this.questions = result.items;
                this.showPaging(result, 0);
            });
    }

}
