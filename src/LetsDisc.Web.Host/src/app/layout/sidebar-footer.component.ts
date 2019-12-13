import { Component, Injector, ViewEncapsulation } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';

@Component({
    templateUrl: './sidebar-footer.component.html',
    selector: 'sidebar-footer'
})
export class SideBarFooterComponent extends AppComponentBase {

    currentYear: number;

    constructor(
        injector: Injector
    ) {
        super(injector);

        this.currentYear = new Date().getFullYear();
    }
}
