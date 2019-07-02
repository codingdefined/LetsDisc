import { Component, Injector, ViewEncapsulation } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';

@Component({
    templateUrl: './topbar.component.html',
    selector: 'top-bar'
})
export class TopBarComponent extends AppComponentBase {

    constructor(
        injector: Injector
    ) {
        super(injector);
    }
}