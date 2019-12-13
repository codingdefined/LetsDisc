import { Component, Injector, ViewEncapsulation } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { MenuItem } from '@shared/layout/menu-item';

@Component({
    templateUrl: './sidebar-nav.component.html',
    selector: 'sidebar-nav'
})
export class SideBarNavComponent extends AppComponentBase {

    menuItems: MenuItem[] = [
        new MenuItem(this.l('Questions'), '', 'question_answer', '/questions'),
        new MenuItem(this.l('Tags'), '', 'local_offer', '/tags'),
        new MenuItem(this.l('Users'), '', 'people', '/users'),
        new MenuItem(this.l('About'), '', 'info', '/about'),
        new MenuItem(this.l('Contact'), '', 'contact_mail', '/contact'),
        new MenuItem(this.l('Privacy'), '', 'security', '/privacy')
    ];

    constructor(
        injector: Injector
    ) {
        super(injector);
    }

    showMenuItem(menuItem): boolean {
        if (menuItem.permissionName) {
            return this.permission.isGranted(menuItem.permissionName);
        }

        return true;
    }
}
