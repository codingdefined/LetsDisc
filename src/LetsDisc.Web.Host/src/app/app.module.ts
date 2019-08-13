import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { JsonpModule } from '@angular/http';
import { HttpClientModule, HttpResponse } from '@angular/common/http';

import { ModalModule } from 'ngx-bootstrap';
import { NgxPaginationModule } from 'ngx-pagination';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';

import { AbpModule } from '@abp/abp.module';

import { ServiceProxyModule } from '@shared/service-proxies/service-proxy.module';
import { SharedModule } from '@shared/shared.module';

import { AboutComponent } from '@app/about/about.component';
import { UsersComponent } from '@app/users/users.component';
import { EditUserComponent } from './users/edit-user/edit-user.component';
import { RolesComponent } from '@app/roles/roles.component';
import { CreateRoleComponent } from '@app/roles/create-role/create-role.component';
import { TenantsComponent } from '@app/tenants/tenants.component';
import { CreateTenantComponent } from './tenants/create-tenant/create-tenant.component';
import { EditTenantComponent } from './tenants/edit-tenant/edit-tenant.component';
import { TopBarComponent } from '@app/layout/topbar.component';
import { TopBarLanguageSwitchComponent } from '@app/layout/topbar-languageswitch.component';
import { SideBarUserAreaComponent } from '@app/layout/sidebar-user-area.component';
import { SideBarNavComponent } from '@app/layout/sidebar-nav.component';
import { SideBarFooterComponent } from '@app/layout/sidebar-footer.component';
import { RightSideBarComponent } from '@app/layout/right-sidebar.component';
import { CreateQuestionComponent } from '@app/posts/create-question/create-question.component';
import { PostsComponent } from './posts/posts.component';
import { CKEditorModule } from '@ckeditor/ckeditor5-angular';
import { TagInputModule } from 'ngx-chips';
import { QuestionDetailComponent } from './posts/question-detail/question-detail.component';
import { CommaSepartedPipe } from './pipes/comma-separted.pipe';
import { TimeAgoPipe } from './pipes/time-ago.pipe';
import { RemoveHtmlTagPipe } from './pipes/remove-html-tag.pipe';
import { TruncateTextPipe } from './pipes/truncate-text.pipe';
import { PostsTagComponent } from './posts/posts-tag/posts-tag.component';
import { PageNotFoundComponent } from './page-not-found/page-not-found.component';
import { TagsComponent } from './tags/tags.component';
import { SocialLoginModule } from "angularx-social-login";
import { UserDetailComponent } from './users/user-detail/user-detail.component';
import { PrivacyComponent } from './privacy/privacy.component';
import { ContactComponent } from './contact/contact.component';
import { SearchPostsComponent } from './search-posts/search-posts.component';
import { QuillModule } from 'ngx-quill';

@NgModule({
    declarations: [
        AppComponent,
        AboutComponent,
        TenantsComponent,
		CreateTenantComponent,
		EditTenantComponent,
        UsersComponent,
        CreateQuestionComponent,
		EditUserComponent,
      	RolesComponent,        
		CreateRoleComponent,
        TopBarComponent,
        TopBarLanguageSwitchComponent,
        SideBarUserAreaComponent,
        SideBarNavComponent,
        SideBarFooterComponent,
        RightSideBarComponent,
        PostsComponent,
        QuestionDetailComponent,
        CommaSepartedPipe,
        TimeAgoPipe,
        PostsTagComponent,
        PageNotFoundComponent,
        TagsComponent,
        UserDetailComponent,
        PrivacyComponent,
        ContactComponent,
        SearchPostsComponent,
        RemoveHtmlTagPipe,
        TruncateTextPipe
    ],
    imports: [
        CommonModule,
        FormsModule,
        HttpClientModule,
        JsonpModule,
        ModalModule.forRoot(),
        AbpModule,
        AppRoutingModule,
        ServiceProxyModule,
        SharedModule,
        NgxPaginationModule,
        CKEditorModule,
        TagInputModule,
        SocialLoginModule,
        QuillModule.forRoot()
    ],
    providers: [
    ]
})
export class AppModule { }
