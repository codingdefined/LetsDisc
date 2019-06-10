import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { AppComponent } from './app.component';
import { AppRouteGuard } from '@shared/auth/auth-route-guard';
import { AboutComponent } from './about/about.component';
import { UsersComponent } from './users/users.component';
import { HomeComponent } from '@app/home/home.component';
import { PostsComponent } from '@app/posts/posts.component';
import { CreateQuestionComponent } from '@app/posts/create-question/create-question.component';
import { QuestionDetailComponent } from '@app/posts/question-detail/question-detail.component';

@NgModule({
    imports: [
        RouterModule.forChild([
            {
                path: '',
                component: AppComponent,
                children: [
                    { path: 'questions', component: PostsComponent },
                    { path: 'questions/ask', component: CreateQuestionComponent },
                    {path: 'questions/:id/:title', component: QuestionDetailComponent},
                    { path: 'users', component: UsersComponent },
                    { path: 'about', component: AboutComponent },
                    { path: 'home', component: HomeComponent },
                    { path: '', redirectTo: '/questions', pathMatch: 'full' },
                ]
            }
        ])
    ],
    exports: [RouterModule]
})
export class AppRoutingModule { }
