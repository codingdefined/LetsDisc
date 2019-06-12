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
  styleUrls: ['./question-detail.component.css', './../tag-theme.scss']
})
export class QuestionDetailComponent extends AppComponentBase implements OnInit {

    question: PostWithVoteInfo;
    answer: SubmitAnswerInput = new SubmitAnswerInput();
    answers: PostWithVoteInfo[] = [];
    id: number;
    items = [];
    editing: boolean = false;
    saving: boolean = false;
    browerRefresh: boolean = false;
    public Editor = ClassicEditor;
    public config = {
        placeholder: 'Type the content here!'
    };

    itemPluralMapping = {
        'answer': {
            '=0': '0 answers',
            '=1': '1 answer',
            'other': '# answers'
        },
        'time': {
            '=0': '0 times',
            '=1': '1 time',
            'other': '# times'
        },
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
                    this.items = result.post.post.tags.split(',');
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
            this.scoreAndVote(this.question, result);
        } else {
            let curAnswer = this.answers.filter(a => a.post.id === result.postId)[0];
            this.scoreAndVote(curAnswer, result);
        }
        
    }

    private scoreAndVote(post: PostWithVoteInfo, result: VoteChangeOutput, ) {
        post.post.score = result.voteCount;
        post.upvote = result.upVote;
        post.downvote = result.downVote;
    }

    saveAnswer(): void {
        this._postService.submitAnswer(this.answer)
            .subscribe((result: PostWithVoteInfo) => {
                this.answers.unshift(result);
                this.notify.info(this.l('Answered'));
            });
    }

    editClicked(): void {
        this.editing = true;
    }

    cancelClick(): void {
        this.editing = false;
    }

    saveQuestion(): void {
        let localItems = [];
        this.items.forEach(function (item) {
            if (item && item.value) {
                localItems.push(item.value);
            } else if (item) {
                localItems.push(item);
            }
        });
        this.question.post.tags = localItems.join();
        this._postService.updateQuestion(this.question.post)
            .subscribe((result: PostWithAnswers) => {
                this.question.post = result.post.post;
                this.editing = false;
            });
    }
}

