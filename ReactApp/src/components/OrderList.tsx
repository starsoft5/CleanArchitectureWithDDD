import { useEffect, useState } from 'react';
import axios from 'axios';

type OrderItem = {
  id: number;
  orderId: number;
  product: string;
  quantity: number;
  price: number;
};

type Order = {
  id: number;
  customerName: string;
  orderDate: string;
  items: OrderItem[];
};

const formatDate = (isoDate: string) =>
  new Date(isoDate).toLocaleDateString();

const OrderList = () => {
  const [orders, setOrders] = useState<Order[]>([]);

  useEffect(() => {
    axios
      .get<Order[]>('http://localhost:7112/api/GetAllOrdersFunction')
      .then((response) => setOrders(response.data))
      .catch((error) => console.error('Error fetching orders:', error));
  }, []);

  return (
    <div className="container mt-4">
      <h2 className="mb-4">Orders</h2>
      {orders.map((order) => (
        <div className="card mb-3" key={order.id}>
          <div className="card-header bg-primary text-white">
            <strong>Customer:</strong> {order.customerName}
          </div>
          <div className="card-body">
            <p>
              <strong>Order Date:</strong> {formatDate(order.orderDate)}
            </p>
            <table className="table table-bordered table-sm">
              <thead className="table-light">
                <tr>
                  <th>Product</th>
                  <th>Quantity</th>
                  <th>Price</th>
                  <th>Line Total</th>
                </tr>
              </thead>
              <tbody>
                {order.items.map((item) => (
                  <tr key={item.id}>
                    <td>{item.product}</td>
                    <td>{item.quantity}</td>
                    <td>${item.price.toFixed(2)}</td>
                    <td>${(item.quantity * item.price).toFixed(2)}</td>
                  </tr>
                ))}
              </tbody>
            </table>
            <div className="text-end">
              <strong>
                Grand Total:{' '}
                $
                {order.items
                  .reduce((sum, item) => sum + item.quantity * item.price, 0)
                  .toFixed(2)}
              </strong>
            </div>
          </div>
        </div>
      ))}
    </div>
  );
};

export default OrderList;
