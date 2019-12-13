import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { switchMap } from 'rxjs/operators';

@Component({
  selector: 'app-posts-tag',
  templateUrl: './posts-tag.component.html',
  styleUrls: ['./posts-tag.component.css']
})
export class PostsTagComponent implements OnInit {
    tag: string;

    constructor(private route: ActivatedRoute) { }

    ngOnInit() {
        this.route.paramMap.subscribe((params) => {
            this.tag = params.get('tag');
        });
    }
}

