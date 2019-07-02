import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { AppComponent } from './app.component';
import { AppRouteGuard } from '@shared/auth/auth-route-guard';
import { AboutComponent } from './about/about.component';
import { UsersComponent } from './users/users.component';
import { PostsComponent } from '@app/posts/posts.component';
import { CreateQuestionComponent } from '@app/posts/create-question/create-question.component';
import { QuestionDetailComponent } from '@app/posts/question-detail/question-detail.component';
import { PostsTagComponent } from '@app/posts/posts-tag/posts-tag.component';
import { PageNotFoundComponent } from '@app/page-not-found/page-not-found.component';
import { TagsComponent } from '@app/tags/tags.component';

@NgModule({
    imports: [
        RouterModule.forChild([
            {
                path: '',
                component: AppComponent,
                children: [
                    {
                        path: 'questions',
                        children: [
                            { path: '', component: PostsComponent },
                            { path: 'tagged/:tag', component: PostsTagComponent },
                            { path: ':id/:title', component: QuestionDetailComponent },
                            { path: 'ask', component: CreateQuestionComponent, canActivate: [AppRouteGuard] },
                        ]
                    },
                    { path: 'users', component: UsersComponent },
                    { path: 'about', component: AboutComponent },
                    { path: 'tags', component: TagsComponent },
                    { path: '', redirectTo: '/questions', pathMatch: 'full' },
                    { path: '**', component: PageNotFoundComponent }
                ]
            }
        ])
    ],
    exports: [RouterModule]
})
export class AppRoutingModule { }
