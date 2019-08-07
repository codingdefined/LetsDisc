import { Component, Injector } from '@angular/core';
import { TagDto, PagedResultDtoOfTagDto, TagServiceProxy } from '@shared/service-proxies/service-proxies';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { finalize } from 'rxjs/operators';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Title } from '@angular/platform-browser';

@Component({
  selector: 'app-tags',
  templateUrl: './tags.component.html',
    styleUrls: ['./tags.component.css'],
    animations: [appModuleAnimation()]
})
export class TagsComponent extends PagedListingComponentBase<TagDto> {

    tags: TagDto[] = [];


  constructor(injector: Injector, private _tagService: TagServiceProxy, private titleService : Title) {
      super(injector);
      this.titleService.setTitle("List of all tags - LetsDisc");
  }

    protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
        this._tagService.getTags(request.skipCount, request.maxResultCount)
        .pipe(finalize(() => {
            finishedCallback()
        }))
        .subscribe((result: PagedResultDtoOfTagDto) => {
            this.tags = result.items;
            this.showPaging(result, pageNumber);
        });
  }

  protected delete(tag: TagDto): void {
  }

}
