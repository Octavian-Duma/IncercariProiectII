import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoginComponent } from './auth/login/login.component';
import { RegisterComponent } from './auth/register/register.component';
import { AuthInterceptor } from './auth/auth.interceptor';
import { ProductListComponent } from './products/product-list/product-list.component';
import { ProductDetailsComponent } from './products/product-details/product-details.component';
import { ProductFormComponent } from './products/product-form/product-form.component';
import { ReactiveFormsModule } from '@angular/forms';
import { RentProductComponent } from './rental/rent-product/rent-product.component';
import { EditAccountComponent } from './account/edit-account/edit-account.component';

@NgModule({
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    CommonModule,
    ReactiveFormsModule,
    ProductDetailsComponent,
    ProductFormComponent,
    RentProductComponent,
    EditAccountComponent,
    ProductListComponent,
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    }
  ],
  bootstrap: [AppComponent],
  declarations: [],
})
export class AppModule { }
