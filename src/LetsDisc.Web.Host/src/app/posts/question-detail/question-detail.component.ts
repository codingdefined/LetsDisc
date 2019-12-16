import { Component, OnInit, Injector } from '@angular/core';
import {
    PostDto,
    PostServiceProxy,
    VoteChangeOutput,
    PostWithVoteInfo,
    SubmitAnswerInput,
    PostWithAnswers
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/app-component-base';
import * as ClassicEditor from '@ckeditor/ckeditor5-build-classic';
import { ActivatedRoute, Router } from '@angular/router';
import { Title, Meta } from '@angular/platform-browser';
import { ShareService } from '@ngx-share/core';

@Component({
  selector: 'app-question-detail',
  templateUrl: './question-detail.component.html',
  styleUrls: ['./question-detail.component.css', './../tag-theme.scss']
})
export class QuestionDetailComponent extends AppComponentBase implements OnInit {

    question: PostWithVoteInfo;
    notEditedQuestion: PostWithVoteInfo;
    answer: SubmitAnswerInput = new SubmitAnswerInput();
    answers: PostWithVoteInfo[] = [];
    id: number;
    items = [];
    editing = false;
    answerEditing = false;
    currentAnswerEditing: PostWithVoteInfo;
    saving = false;
    browerRefresh = false;
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
        private route: ActivatedRoute,
        private router: Router,
        private titleService: Title,
        private meta: Meta,
        public share: ShareService
    ) {
        super(injector);
    }

    ngOnInit() {
        this.route.paramMap.subscribe((params) => {
            this.id = +params.get('id');
            this.getNewPost(this.id);
        });
    }

    private getNewPost(id: number) {
        this._postService.getPost(id)
            .subscribe((result: PostWithAnswers) => {
                this.question = result.post;
                this.answers = result.answers;
                this.items = result.post.post.tags.split(',');
                this.titleService.setTitle(this.items[0] + ' - ' + result.post.post.title);
                this.meta.updateTag({ name: 'description', content: result.post.post.body.replace(/<[^>]*>/g, '').substring(0, 160) });
                this.answer.title = result.post.post.title;
            });
        this.answer.questionId = id;
    }

    canVote(post: PostDto): boolean {
        if (!this.appSession.isLoggedIn) {
            this.notify.info(this.l('Please login'));
            return false;
        } else if (this.appSession.userId === post.creatorUserId) {
            this.notify.info(this.l('You cannot vote on your own post'));
            return false;
        } else {
            return true;
        }
    }

    voteUp(post: PostDto) {
        if (this.canVote(post)) {
            this._postService.postVoteUp(post.id)
                .subscribe((result: VoteChangeOutput) => {
                    this.assignScoreData(result);
                    this.notify.info(this.l('Voted'));
                });
        }
    }

    voteDown(post: PostDto) {
        if (this.canVote(post)) {
            this._postService.postVoteDown(post.id)
                .subscribe((result: VoteChangeOutput) => {
                    this.assignScoreData(result);
                    this.notify.info(this.l('Voted'));
                });
        }
    }

    assignScoreData(result: VoteChangeOutput) {
        if (result.postTypeId === 1 && result.postId === this.question.post.id) {
            this.scoreAndVote(this.question, result);
        } else {
            const curAnswer = this.answers.filter(a => a.post.id === result.postId)[0];
            this.scoreAndVote(curAnswer, result);
        }
    }

    private scoreAndVote(post: PostWithVoteInfo, result: VoteChangeOutput, ) {
        post.post.score = result.voteCount;
        post.upvote = result.upVote;
        post.downvote = result.downVote;
    }

    saveAnswer(): void {
        if (this.appSession.isLoggedIn) {
            this._postService.submitAnswer(this.answer)
                .subscribe((result: PostWithVoteInfo) => {
                    this.answers.unshift(result);
                    this.notify.info(this.l('Answered'));
                });
        } else {
            this.notify.info(this.l('Please login'));
        }
    }

    editClicked(): void {
        this.notEditedQuestion = JSON.parse(JSON.stringify(this.question));
        this.editing = true;
    }

    deleteClicked(): void {
        abp.message.confirm(
            'Delete question "' + this.question.post.title + '"?',
            (result: boolean) => {
                if (result) {
                    this._postService.delete(this.question.post.id)
                        .subscribe(() => {
                            abp.notify.info('Deleted Question: ' + this.question.post.title);
                            this.router.navigate['/questions'];
                        });
                }
            }
        );
    }

    editAnswerClicked(answer: PostWithVoteInfo): void {
        this.answerEditing = true;
        this.currentAnswerEditing = JSON.parse(JSON.stringify(answer));
    }

    cancelClick(): void {
        this.editing = false;
        this.question = this.notEditedQuestion;
    }

    answerCancelClick(): void {
        this.answerEditing = false;
        this.currentAnswerEditing = null;
    }

    updateAnswer(): void {
        const index = this.answers.findIndex((item) => item.post.id === this.currentAnswerEditing.post.id);
        this._postService.updateAnswer(this.currentAnswerEditing.post)
            .subscribe((result: PostWithVoteInfo) => {
                this.answers[index] = result;
                this.answers = JSON.parse(JSON.stringify(this.answers));
                this.notify.info('Answer Updated');
                this.answerEditing = false;
                this.currentAnswerEditing = null;
            });
    }

    saveQuestion(): void {
        const localItems = [];
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

