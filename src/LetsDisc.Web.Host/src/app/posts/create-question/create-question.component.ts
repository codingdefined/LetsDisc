import { Component, OnInit, Injector } from '@angular/core';
import { CreatePostDto, PostServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/app-component-base';
import * as ClassicEditor from '@ckeditor/ckeditor5-build-classic';
import { TagInputModule } from 'ngx-chips';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-create-question',
  templateUrl: './create-question.component.html',
  styleUrls: ['./create-question.component.css', './../tag-theme.scss']
})
export class CreateQuestionComponent extends AppComponentBase implements OnInit {

    question: CreatePostDto = new CreatePostDto();
    items = [];
    saving: boolean = false;
    public Editor = ClassicEditor;
    public config = {
       placeholder: 'Type the content here!'
    };


    constructor(
        injector: Injector,
        private _postService: PostServiceProxy,
    ) { 
        super(injector);
        this.question.postTypeId = 1;
    }

    ngOnInit() {
    }

    save(): void {
        let localItems = [];
        this.items.forEach(function(item) {
          localItems.push(item.value);
        });
        this.question.tags = localItems.join();
        this.saving = true;
        this._postService.create(this.question)
            .pipe(finalize(() => { this.saving = false; }))
            .subscribe(() => {
                this.notify.info(this.l('SavedSuccessfully'));
            });
    }

}