import { Component, OnInit } from '@angular/core';
import { PagedListingComponentBase, PagedRequestDto } from 'shared/paged-listing-component-base';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-questions',
  templateUrl: './questions.component.html',
  styleUrls: ['./questions.component.css']
})
export class QuestionsComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

}
