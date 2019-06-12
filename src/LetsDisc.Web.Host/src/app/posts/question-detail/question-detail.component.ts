import { Component, OnInit, Injector } from '@angular/core';
import { PostDto, PostServiceProxy, VoteChangeOutput, PostWithVoteInfo, SubmitAnswerInput, PostWithAnswers } from '@shared/service-proxies/service-proxies';
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

    question: PostWithVoteInfo;
    answer: SubmitAnswerInput = new SubmitAnswerInput();
    answers: PostWithVoteInfo[] = [];
    id: number;
    upvote: boolean = false;
    downvote: boolean = false;
    saving: boolean = false;
    public Editor = ClassicEditor;
    public config = {
        placeholder: 'Type the content here!'
    };

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
                (result: PostWithAnswers) => {
                    this.question = result.post;
                    this.answers = result.answers;
            });
        this.answer.questionId = this.id;
    }

    voteUp(postId: number) {
        this._postService.postVoteUp(postId)
            .subscribe((result: VoteChangeOutput) => {
                this.assignScoreData(result);
                this.notify.info(this.l('Voted'));
            });
    }

    voteDown(postId: number) {
        this._postService.postVoteDown(postId)
            .subscribe((result: VoteChangeOutput) => {
                this.assignScoreData(result);
                this.notify.info(this.l('Voted'));
            });
    }

    assignScoreData(result: VoteChangeOutput) {
        if (result.postTypeId == 1 && result.postId == this.question.post.id) {
            this.question.post.score = result.voteCount;
            this.question.upvote = result.upVote;
            this.question.downvote = result.downVote;
        } else {
            let curAnswer = this.answers.filter(a => a.post.id === result.postId)[0];
            curAnswer.post.score = result.voteCount;
            curAnswer.upvote = result.upVote;
            curAnswer.downvote = result.downVote;
        }
        
    }

    saveAnswer(): void {
        this._postService.submitAnswer(this.answer)
            .subscribe((result: PostDto) => {
                console.log(result);
                this.notify.info(this.l('Answered'));
            });
    }
}

