import { Component, Injector, AfterViewInit } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { Title } from '@angular/platform-browser';

@Component({
    templateUrl: './about.component.html',
    animations: [appModuleAnimation()]
})
export class AboutComponent extends AppComponentBase {

    constructor(
        injector: Injector,
        private titleService: Title
    ) {
        super(injector);
        this.titleService.setTitle("About - LetsDisc");
    }
}