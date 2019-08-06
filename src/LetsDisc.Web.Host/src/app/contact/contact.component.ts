import { Component, OnInit } from '@angular/core';
import { Title, Meta } from '@angular/platform-browser';

@Component({
  selector: 'app-contact',
  templateUrl: './contact.component.html'
})
export class ContactComponent implements OnInit {

    constructor(private titleService: Title, private meta: Meta) {
        this.titleService.setTitle("Contact - LetsDisc");
        this.meta.updateTag({ name: 'description', content: "You may contact us with your feedback and suggestions at our email address: info AT letsdiscuss DOT com." }); 
    }

  ngOnInit() {
  }

}
