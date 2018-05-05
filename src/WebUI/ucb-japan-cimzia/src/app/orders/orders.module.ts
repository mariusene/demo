import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OrdersComponent } from './orders.component';
import { CoreModule } from '../core/core.module';
import { MaterialModule } from '../material.module'


@NgModule({
  imports: [
    CommonModule,
    CoreModule,
    MaterialModule
  ],
  declarations: [OrdersComponent],
  exports: [OrdersComponent]
})
export class OrdersModule { }
