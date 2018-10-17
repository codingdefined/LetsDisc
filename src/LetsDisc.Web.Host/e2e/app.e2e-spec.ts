import { LetsDiscTemplatePage } from './app.po';

describe('LetsDisc App', function() {
  let page: LetsDiscTemplatePage;

  beforeEach(() => {
    page = new LetsDiscTemplatePage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('app works!');
  });
});
