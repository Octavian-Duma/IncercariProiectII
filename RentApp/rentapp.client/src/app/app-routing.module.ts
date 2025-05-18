import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './auth/login/login.component';
import { RegisterComponent } from './auth/register/register.component';
import { ProductListComponent } from './products/product-list/product-list.component';
import { ProductDetailsComponent } from './products/product-details/product-details.component';
import { ProductFormComponent } from './products/product-form/product-form.component';
import { AccountComponent } from './account/account.component';
import { EditAccountComponent } from './account/edit-account/edit-account.component';
import { MyRentalsComponent } from './rental/myrentals/myrentals.component';
import { RentProductComponent } from './rental/rent-product/rent-product.component';

const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'products', component: ProductListComponent },
  { path: 'products/:id', component: ProductDetailsComponent },
  { path: 'create', component: ProductFormComponent },
  { path: 'account', component: AccountComponent },
  { path: 'edit-account', component: EditAccountComponent },
  { path: 'rentals/my', component: MyRentalsComponent },
  { path: 'rent/:id', component: RentProductComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
