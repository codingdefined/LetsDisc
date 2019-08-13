import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'removeHtmlTag'
})
export class RemoveHtmlTagPipe implements PipeTransform {

  transform(val: any): string[] {
    return val.replace(/<[^>]*>/g, '');
  }
}