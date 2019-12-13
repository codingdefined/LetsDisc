import { NgModule } from '@angular/core';
import { Routes, RouterModule, ExtraOptions } from '@angular/router';

const routes: Routes = [
    {
        path: '',
        loadChildren: 'app/app.module#AppModule', // Lazy load account module
        data: { preload: true }
    }
];

const routerOptions: ExtraOptions = {
    anchorScrolling: 'enabled'
};

@NgModule({
    imports: [RouterModule.forRoot(routes, routerOptions)],
    exports: [RouterModule],
    providers: []
})
export class RootRoutingModule { }
