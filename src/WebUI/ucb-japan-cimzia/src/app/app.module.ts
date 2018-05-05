import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { OrdersModule } from './orders/orders.module';
import { HttpClientModule } from '@angular/common/http';
import { MaterialModule } from './material.module';

import { AppComponent } from './app.component';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    OrdersModule,
    HttpClientModule,
    MaterialModule
  ],
  providers: [],
  bootstrap: [ AppComponent ]
})
export class AppModule { }
