import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PostsTagComponent } from './posts-tag.component';

describe('PostsTagComponent', () => {
  let component: PostsTagComponent;
  let fixture: ComponentFixture<PostsTagComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PostsTagComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PostsTagComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
