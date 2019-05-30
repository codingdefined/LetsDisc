import { Component, Injector, ViewChild } from '@angular/core';
import { PagedListingComponentBase, PagedRequestDto } from 'shared/paged-listing-component-base';
import { QuestionServiceProxy, QuestionDto, PagedResultDtoOfQuestionDto } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { EditQuestionComponent } from 'app/questions/edit-question/edit-question.component';

@Component({
  selector: 'app-questions',
  templateUrl: './questions.component.html',
  styleUrls: ['./questions.component.css'],
  animations: [appModuleAnimation()]
})
export class QuestionsComponent extends PagedListingComponentBase<QuestionDto> {

    @ViewChild('editQuestionModal') editQuestionModal: EditQuestionComponent;

    questions: QuestionDto[] = [];

    constructor(injector: Injector, private _questionService: QuestionServiceProxy) {
        super(injector);
    }

    protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
        this._questionService.getQuestions(request.maxResultCount, request.skipCount, request.sorting)
            .pipe(finalize(() => {
                finishedCallback()
            }))
            .subscribe((result: PagedResultDtoOfQuestionDto) => {
                this.questions = result.items;
                this.showPaging(result, pageNumber);
            });
    }

    protected delete(question: QuestionDto): void {
        abp.message.confirm(
            "Delete question '" + question.title + "'?",
            (result: boolean) => {
                if (result) {
                    this._questionService.deleteQuestion(question.id)
                        .subscribe(() => {
                            abp.notify.info("Deleted Question: " + question.title);
                            this.refresh();
                        });
                }
            }
        );
    }

    editQuestion(question: QuestionDto): void {
        this.editQuestionModal.show(question.id);
    }

}
