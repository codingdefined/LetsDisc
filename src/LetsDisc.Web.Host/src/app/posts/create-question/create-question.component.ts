import { Component, OnInit, Injector } from '@angular/core';
import { PostDto, PostServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/app-component-base';

@Component({
  selector: 'app-create-question',
  templateUrl: './create-question.component.html',
  styleUrls: ['./create-question.component.css']
})
export class CreateQuestionComponent extends AppComponentBase implements OnInit {

    question: PostDto = new PostDto();

    constructor(
        injector: Injector,
        private _postService: PostServiceProxy,
    ) {
        super(injector);
    }

  ngOnInit() {
  }

}
