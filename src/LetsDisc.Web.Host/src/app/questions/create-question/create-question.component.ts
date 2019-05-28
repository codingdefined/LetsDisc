import { Component, ViewChild, Injector, Output, EventEmitter, ElementRef, OnInit } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap';
import { QuestionServiceProxy, CreateQuestionInput } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/app-component-base';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'create-question-modal',
  templateUrl: './create-question.component.html'
})
export class CreateQuestionComponent extends AppComponentBase implements OnInit {

    @ViewChild('createQuestionModal') modal: ModalDirective;
    @ViewChild('modalContent') modalContent: ElementRef;

    @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

    active: boolean = false;
    saving: boolean = false;
    question: CreateQuestionInput = null;

    constructor(
        injector: Injector,
        private _questionService: QuestionServiceProxy,
    ) {
        super(injector);
    }

    ngOnInit(): void {
        
    }

    show(): void {
        this.active = true;
        this.modal.show();
        this.question = new CreateQuestionInput();
    }

    onShown(): void {
        $.AdminBSB.input.activate($(this.modalContent.nativeElement));
    }

    save(): void {
        this.saving = true;
        this._questionService.createQuestion(this.question)
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
