import { Component, OnInit, Injector } from '@angular/core';
import { PostDto, PostServiceProxy, VoteChangeOutput, PostWithVoteInfo } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/app-component-base';
import * as ClassicEditor from '@ckeditor/ckeditor5-build-classic';
import { TagInputModule } from 'ngx-chips';
import { finalize } from 'rxjs/operators';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-question-detail',
  templateUrl: './question-detail.component.html',
  styleUrls: ['./question-detail.component.css']
})
export class QuestionDetailComponent extends AppComponentBase implements OnInit {

    question: PostDto;
    items = [];
    id: number;
    upvote: boolean = false;
    downvote: boolean = false;
    saving: boolean = false;
    public Editor = ClassicEditor;

    constructor(
        injector: Injector,
        private _postService: PostServiceProxy,
        private route: ActivatedRoute
    ) { 
        super(injector);
    }

    ngOnInit() {
        this.id = +this.route.snapshot.paramMap.get("id");
        this._postService.getPost(this.id)
            .subscribe(
            (result: PostWithVoteInfo) => {
                this.question = result.post;
                this.upvote = result.upvote;
                this.downvote = result.downvote;
            });
    }

    voteUp() {
        this._postService.postVoteUp(this.id)
            .subscribe((result: VoteChangeOutput) => {
                this.assignData(result);
                this.notify.info(this.l('Voted'));
            });
    }

    voteDown() {
        this._postService.postVoteDown(this.id)
            .subscribe((result: VoteChangeOutput) => {
                this.assignData(result);
                this.notify.info(this.l('Voted'));
            });
    }

    assignData(result: VoteChangeOutput) {
        this.question.score = result.voteCount;
        this.upvote = result.upVote;
        this.downvote = result.downVote;
    }
}

