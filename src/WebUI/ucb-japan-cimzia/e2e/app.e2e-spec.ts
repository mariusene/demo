import { AppPage } from './app.po';

describe('ucb-japan-cimzia App', () => {
  let page: AppPage;

  beforeEach(() => {
    page = new AppPage();
  });

  it('should display welcome message', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('Welcome to ucbjc!');
  });
});
