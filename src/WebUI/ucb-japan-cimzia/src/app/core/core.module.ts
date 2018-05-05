import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BaseUriService } from './baseuri.service';
import { APIOrdersService } from './api.orders.service';

@NgModule({
  imports: [
    CommonModule
  ],
  declarations: [],
  providers: [
    BaseUriService, 
    APIOrdersService 
  ]
})
export class CoreModule { }
