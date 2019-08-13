import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'commaSeparted'
})
export class CommaSepartedPipe implements PipeTransform {

  transform(val: any): string[] {
    return val.split(',');
  }
}