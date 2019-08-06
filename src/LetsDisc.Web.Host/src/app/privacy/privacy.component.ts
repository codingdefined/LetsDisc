import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';

@Component({
  selector: 'app-privacy',
  templateUrl: './privacy.component.html'
})
export class PrivacyComponent implements OnInit {

    constructor(private titleService: Title) {
        this.titleService.setTitle("Privacy - LetsDisc");
    }

  ngOnInit() {
  }

}
