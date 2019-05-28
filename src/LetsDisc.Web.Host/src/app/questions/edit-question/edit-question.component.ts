import { Component, ViewChild, Injector, Output, EventEmitter, ElementRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { QuestionServiceProxy, QuestionDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/app-component-base';
import { finalize } from 'rxjs/operators';

@Component({
    selector: 'edit-question-modal',
    templateUrl: './edit-question.component.html'
})
export class EditQuestionComponent extends AppComponentBase {

    @ViewChild('editQuestionModal') modal: ModalDirective;
    @ViewChild('modalContent') modalContent: ElementRef;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active: boolean = false;
    saving: boolean = false;

    question: QuestionDto = null;

    constructor(
        injector: Injector,
        private _questionService: QuestionServiceProxy
    ) {
        super(injector);
    }

    show(id: number): void {

        this._questionService.getQuestion(null, id)
            .subscribe(
            (result) => {
                this.question = result.question;
                this.active = true;
                this.modal.show();
            }
            );
    }

    onShown(): void {
        $.AdminBSB.input.activate($(this.modalContent.nativeElement));
    }

    save(): void {

        this.saving = true;
        this._questionService.editQuestion(this.question)
            .pipe(finalize(() => { this.saving = false; }))
            .subscribe(() => {
                this.notify.info(this.l('SavedSuccessfully'));
                this.close();
                this.modalSave.emit(null);
            });
    }

    close(): void {
        this.active = false;
        this.modal.hide();
    }
}
