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
import { UserDetailComponent } from '@app/users/user-detail/user-detail.component';
import { EditUserComponent } from '@app/users/edit-user/edit-user.component';

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
                            { path: 'ask', component: CreateQuestionComponent, canActivate: [AppRouteGuard] },
                            { path: ':id', component: QuestionDetailComponent },
                            { path: ':id/:title', component: QuestionDetailComponent }
                        ]
                    },
                    {
                        path: 'users',
                        children: [
                            { path: '', component: UsersComponent },
                            { path: ':id', component: UserDetailComponent },
                            { path: ':id/edit', component: EditUserComponent },
                            { path: ':id/:name', component: UserDetailComponent }
                        ]
                    },
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
