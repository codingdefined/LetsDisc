import { Component, OnInit, Injector } from '@angular/core';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { PostServiceProxy, PostDto, PagedResultDtoOfPostDto } from '@shared/service-proxies/service-proxies';
import { Title } from '@angular/platform-browser';
import { ActivatedRoute, Router } from '@angular/router';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-search-posts',
  templateUrl: './search-posts.component.html',
  styleUrls: ['./search-posts.component.css']
})
export class SearchPostsComponent extends PagedListingComponentBase<PostDto>{

    posts: PostDto[] = [];
    itemPluralMapping = {
        'answer': {
            '=0': 'answers',
            '=1': 'answer',
            'other': 'answers'
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

    constructor(injector: Injector, private _postService: PostServiceProxy, private titleService: Title, private route: ActivatedRoute, private router: Router) {
        super(injector);
    }

    protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
        this.route.queryParamMap.subscribe(urlParams => this.searchString = urlParams.get('q'));
        if (this.searchString !== '') {
            this._postService.getSearchPosts(this.sortByValue, request.skipCount, request.maxResultCount, this.searchString)
                .pipe(finalize(() => {
                    finishedCallback()
                }))
                .subscribe((result: PagedResultDtoOfPostDto) => {
                    this.posts = result.items;
                    this.showPaging(result, pageNumber);
                });
            this.titleService.setTitle("LetsDisc - Posts containing '" + this.searchString + "'");
        } else {
            this.router.navigate(['/questions']);
            abp.notify.info("Search String is Required");
        }

    }

    protected delete(post: PostDto): void {
    }

    public sortBy(value: string): void {
        this.sortByValue = value;
        if (this.searchString !== '') {
            this._postService.getSearchPosts(value, 0, 10, this.searchString)
                .subscribe((result: PagedResultDtoOfPostDto) => {
                    this.posts = result.items;
                    this.showPaging(result, 0);
                });
        } else {
            abp.notify.info("Search String is Required");
        }
        
    }

    public searchPosts(): void {
        if (this.searchString !== '') {
            this.router.navigate(
                [],
                {
                    relativeTo: this.route,
                    queryParams: { q: this.searchString },
                    queryParamsHandling: 'merge'
                });
            this._postService.getSearchPosts(this.sortByValue, 0, 10, this.searchString)
                .subscribe((result: PagedResultDtoOfPostDto) => {
                    this.posts = result.items;
                    this.showPaging(result, 0);
                });
        } else {
            abp.notify.info("Search String is Required");
        }
    }

}

