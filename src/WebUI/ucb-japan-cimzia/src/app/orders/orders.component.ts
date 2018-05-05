import { Component, OnInit } from '@angular/core';
import { APIOrdersService } from '../core/api.orders.service';
import { MatTableDataSource } from '@angular/material';

@Component({
  selector: 'ucbjc-orders',
  templateUrl: './orders.component.html',
  styleUrls: ['./orders.component.scss'],
  providers: [ ]
})
export class OrdersComponent implements OnInit {

  displayedColumns = ['id', 'merchand', 'customer'];
  dataSource: any;
  orders : api.OrderMessage[];


  constructor(private orderService: APIOrdersService) { }

  ngOnInit() {
    this.orderService.getOrders()
      .subscribe(orders => {
        this.orders = orders;
        this.dataSource = new MatTableDataSource(orders);
      });
  }

  addOrder() {
    console.log("send new order");
    this.orderService.addOrder({id:10, merchand:"Amazon", customer: {firstName: "Marius", lastName:"Ene"}})
      .subscribe( res => console.log(res));
  }

  updateOrder() {
    console.log("update order");
    this.orderService.updateOrder(10, {id:10, merchand:"Amazon", customer: {firstName: "Marius", lastName:"Ene"}})
      .subscribe( res => console.log(res));
  }

  deleteOrder(orderId:number) {
    console.log("delete order");
    this.orderService.deleteOrder(orderId)
      .subscribe( res => console.log(res));
  }
}
