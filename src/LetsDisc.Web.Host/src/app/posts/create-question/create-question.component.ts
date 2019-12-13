import { Component, OnInit, Injector } from '@angular/core';
import { CreatePostDto, PostServiceProxy, PostDto, UserServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/app-component-base';
import * as ClassicEditor from '@ckeditor/ckeditor5-build-classic';
import { finalize } from 'rxjs/operators';
import { Router } from '@angular/router';
import { Title } from '@angular/platform-browser';
import { AppConsts } from '@shared/AppConsts';

@Component({
  selector: 'app-create-question',
  templateUrl: './create-question.component.html',
  styleUrls: ['./create-question.component.css', './../tag-theme.scss']
})
export class CreateQuestionComponent extends AppComponentBase implements OnInit {

    question: CreatePostDto = new CreatePostDto();
    items = [];
    saving = false;
    quill: any = null;
    public Editor = ClassicEditor;
    public config = {
       placeholder: 'Type the content here!'
    };


    constructor(
        injector: Injector,
        private _postService: PostServiceProxy,
        private router: Router,
        private titleService: Title,
        private _userService: UserServiceProxy,
    ) {
        super(injector);
        this.question.postTypeId = 1;
        this.titleService.setTitle('Ask a question - LetsDisc');
    }

    ngOnInit() {
    }

    save(): void {
        const localItems = [];
        this.items.forEach(function(item) {
          localItems.push(item.value);
        });
        this.question.tags = localItems.join();
        this.saving = true;
        this._postService.create(this.question)
            .pipe(finalize(() => { this.saving = false; }))
            .subscribe((result: PostDto) => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.router.navigate(['questions', result.id, result.title.replace(' ', '-')]);
            });
    }

    getEditorInstance(quill: any) {
        const toolbar = quill.getModule('toolbar');
        toolbar.addHandler('image', this.imageHandler.bind(this));
        this.quill = quill;
    }

    private imageHandler() {
        const self = this;
        if (this.quill != null) {
            const range = this.quill.getSelection();
            if (range != null) {
                const input = document.createElement('input');
                input.setAttribute('type', 'file');
                input.setAttribute('accept', 'image/*');
                input.addEventListener('change', () => {
                    if (input.files != null) {
                        const fileReader: FileReader = new FileReader();
                        fileReader.readAsDataURL(input.files[0]);
                        fileReader.onloadend = function () {
                            if (fileReader.readyState === 2) {
                                self._userService.uploadProfilePicture({ data: input.files[0], fileName: input.files[0].name }, 1)
                                    .subscribe((result: string) => {
                                        self.quill.insertEmbed(range.index, 'image', self.createImgPath(result));
                                    });
                            }
                        }
                    };
                });
                input.click();
            }
        }
    }

    createImgPath = (serverPath: string) => {
        if (serverPath) {
            return AppConsts.remoteServiceBaseUrl + `/${serverPath}?timeStamp=${Date.now()}`;
        }
        return '';
    }

}
