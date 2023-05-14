
import { Selector, ClientFunction } from 'testcafe';

fixture `Forgot Password`
    .page `http://localhost:5000/Account/ForgotPassword`;

const submitButton = Selector('input[type="submit"]');
const loginField = Selector('input[name="login"]');
const passwordField = Selector('input[name="password"]');

test('Forgot Password - Submitting valid credentials', async t => {
    await t
        .typeText(loginField, 'username')
        .typeText(passwordField, 'password')
        .click(submitButton)
        .expect(ClientFunction(() => window.location.href)()).contains('/Account/ForgotPasswordConfirmation');
});

test('Forgot Password - Submitting invalid credentials', async t => {
    await t
        .typeText(loginField, 'invalid')
        .typeText(passwordField, 'credentials')
        .click(submitButton)
        .expect(Selector('div.text-danger').innerText).contains('Invalid login attempt.');
});

test('Forgot Password - Validation errors', async t => {
    await t
        .click(submitButton)
        .expect(loginField.nextSibling('span.text-danger').innerText).contains('The Login field is required.')
        .expect(passwordField.nextSibling('span.text-danger').innerText).contains('The Password field is required.');
});
import { expect } from 'chai';
import { Builder, Capabilities, By } from 'selenium-webdriver';

const driver = new Builder().withCapabilities(Capabilities.chrome()).build();

describe('Registration page', function () {
  it('should load successfully', async function () {
    await driver.get('http://localhost:3000/Account/Register');
    const title = await driver.getTitle();
    expect(title).to.equal('Реєстрація - Minecraft Mods (Real-Mining)');
  });

  it('should register a new user successfully', async function () {
    await driver.get('http://localhost:3000/Account/Register');
    await driver.findElement(By.name('FirstName')).sendKeys('John');
    await driver.findElement(By.name('LastName')).sendKeys('Doe');
    await driver.findElement(By.name('RoleID')).sendKeys('User');
    await driver.findElement(By.name('Login')).sendKeys('johndoe');
    await driver.findElement(By.name('Password')).sendKeys('password');
    await driver.findElement(By.name('ConfirmPassword')).sendKeys('password');
    await driver.findElement(By.css('.btn-primary')).click();
    const title = await driver.getTitle();
    expect(title).to.equal('Home Page - Minecraft Mods (Real-Mining)');
  });

  it('should display error message if password and confirm password do not match', async function () {
    await driver.get('http://localhost:3000/Account/Register');
    await driver.findElement(By.name('FirstName')).sendKeys('John');
    await driver.findElement(By.name('LastName')).sendKeys('Doe');
    await driver.findElement(By.name('RoleID')).sendKeys('User');
    await driver.findElement(By.name('Login')).sendKeys('johndoe');
    await driver.findElement(By.name('Password')).sendKeys('password');
    await driver.findElement(By.name('ConfirmPassword')).sendKeys('invalid');
    await driver.findElement(By.css('.btn-primary')).click();
    const errorMsg = await driver.findElement(By.css('.text-danger')).getText();
    expect(errorMsg).to.equal('The password and confirmation password do not match.');
  });
});

describe('Create page', () => {
    it('should display the create page', async () => {
      await page.goto('http://localhost:3000/create');
      await expect(page).toMatch('Files');
    });
  });
  
describe('Create page', () => {
    it('should create a new file', async () => {
      await page.goto('http://localhost:3000/create');
  
      const modeVersionInput = await page.$('input[name="ModeVesrion"]');
      await modeVersionInput.type('1.0.0');
  
      const gameVersionInput = await page.$('input[name="GameVersion"]');
      await gameVersionInput.type('1.0.0');
  
      const fileInput = await page.$('input[type="file"]');
      await fileInput.uploadFile('./testfile.txt');
  
      const submitButton = await page.$('input[type="submit"]');
      await submitButton.click();
  
      await expect(page).toMatch('Back to List');
    });
  });
    

  describe('Create page', () => {
    it('should display an error message if required fields are not filled out', async () => {
      await page.goto('http://localhost:3000/create');
  
      const submitButton = await page.$('input[type="submit"]');
      await submitButton.click();
  
      await expect(page).toMatch('The ModeVesrion field is required.');
      await expect(page).toMatch('The GameVersion field is required.');
    });
  });
  it('should display delete button', async () => {
    await page.navigateTo('delete-page-url');
    expect(await page.isElementDisplayed('input[type="submit"][value="Delete"]')).toBe(true);
  });
  it('should display title of the file', async () => {
    await page.navigateTo('delete-page-url');
    expect(await page.getElementText('dt:contains("Title") + dd')).toEqual('file-title');
  });
  it('should display game version of the file', async () => {
    await page.navigateTo('delete-page-url');
    expect(await page.getElementText('dt:contains("GameVersion") + dd')).toEqual('file-game-version');
  });
        

describe('Login page', () => {
    beforeEach(() => {
      // Visit the login page before each test
      cy.visit('/Account/Login');
    });
  
    it('displays the login form', () => {
      // Ensure the login form is displayed
      cy.get('form').should('exist');
      cy.get('form').should('have.attr', 'asp-action', 'Login');
      cy.get('form').should('have.attr', 'asp-controller', 'Account');
    });
  
    it('requires a username and password to log in', () => {
      // Attempt to log in without entering any credentials
      cy.get('form').submit();
      // Ensure validation messages are displayed for both fields
      cy.get('div.validation-summary-errors').should('exist');
      cy.get('div.validation-summary-errors').contains('The Login field is required.');
      cy.get('div.validation-summary-errors').contains('The Password field is required.');
    });
  
    it('displays an error message for invalid login credentials', () => {
      // Attempt to log in with an invalid username and password
      cy.get('input[name="Login"]').type('invalid_username');
      cy.get('input[name="Password"]').type('invalid_password');
      cy.get('form').submit();
      // Ensure an error message is displayed
      cy.get('div.validation-summary-errors').should('exist');
      cy.get('div.validation-summary-errors').contains('Invalid login attempt.');
    });
  
    it('redirects to the home page after a successful login', () => {
      // Log in with valid credentials
      cy.get('input[name="Login"]').type('valid_username');
      cy.get('input[name="Password"]').type('valid_password');
      cy.get('form').submit();
      // Ensure the home page is displayed
      cy.url().should('include', '/');
      cy.contains('Minecraft Mods (Real-Mining)');
      cy.contains('Home');
    });
  });
  