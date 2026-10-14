import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/useAuth';

interface CartItem {
    productId: number;
    title: string;
    price: number;
    quantity: number;
    city: string;
}

export default function CartPage() {
    const [cartItems, setCartItems] = useState<CartItem[]>([]);
    const [total, setTotal] = useState(0);
    const { user, logout } = useAuth();
    const navigate = useNavigate();

    // Cargar carrito desde localStorage o estado inicial
    useEffect(() => {
        const stored = localStorage.getItem('compra-merce-cart');
        if (stored) {
            setCartItems(JSON.parse(stored));
        }
        calcTotal();
    }, []);

    const calcTotal = () => {
        const sum = cartItems.reduce((acc, item) => acc + item.price * item.quantity, 0);
        setTotal(sum);
    };

    const addToCart = (product: CartItem) => {
        // Verificar si el producto ya está en el carrito
        const existing = cartItems.find((item) => item.productId === product.productId);

        if (existing) {
            setCartItems(
                cartItems.map((item) =>
                    item.productId === product.productId
                        ? { ...item, quantity: item.quantity + 1 }
                        : item
                )
            );
        } else {
            setCartItems([
                ...cartItems,
                {
                    productId: product.productId,
                    title: product.title,
                    price: product.price,
                    quantity: 1,
                    city: product.city,
                },
            });
        }

        // Guardar en localStorage
        localStorage.setItem('compra-merce-cart', JSON.stringify(cartItems));
        calcTotal();
    };

    const removeFromCart = (productId: number) => {
        setCartItems(cartItems.filter((item) => item.productId !== productId));
        localStorage.setItem('compra-merce-cart', JSON.stringify(cartItems));
        calcTotal();
    };

    const clearCart = () => {
        setCartItems([]);
        localStorage.removeItem('compra-merce-cart');
        calcTotal();
    };

    return (
        <div style={{
            maxWidth: '900px',
            margin: '0 auto',
            padding: '2rem',
        }}>
            <h1 style={{ marginBottom: '1.5rem' }}>
                Mi Carrito de Compras
            </h1>

            {/* Resumen del carrito */}
            {cartItems.length === 0 ? (
                <div style={{
                    padding: '2rem',
                    textAlign: 'center',
                    color: '#6b7280',
                    backgroundColor: '#f9fafb',
                    borderRadius: '8px',
                    marginBottom: '1.5rem',
                }}>
                    <p>Tu carrito está vacío</p>
                    <p style={{ marginTop: '0.5rem' }}>
                        Agrega productos desde la catálogo para comenzar a comprar
                    </p>
                </div>
                ) : (
                    <>
                        <div style={{ overflowX: 'auto', marginBottom: '1.5rem' }}>
                            <table style={{ width: '100%', borderCollapse: 'collapse' }}>
                                <thead>
                                    <tr style={{ backgroundColor: '#f3f4f6' }}>
                                        <th style={{ padding: '0.75rem', textAlign: 'left' }}>Producto</th>
                                        <th style={{ padding: '0.75rem', textAlign: 'left' }}>Precio</th>
                                        <th style={{ padding: '0.75rem', textAlign: 'left' }}>Cantidad</th>
                                        <th style={{ padding: '0.75rem', textAlign: 'left' }}>Subtotal</th>
                                        <th style={{ padding: '0.75rem', textAlign: 'left' }}>Acciones</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {cartItems.map((item) => (
                                        <tr key={item.productId} style={{ borderBottom: '1px solid #e5e7eb' }}>
                                            <td style={{ padding: '0.75rem' }}>
                                                <strong>{item.title}</strong>
                                            </td>
                                            <td style={{ padding: '0.75rem' }}>
                                                COP {item.price.toLocaleString()}
                                            </td>
                                            <td style={{ padding: '0.75rem' }}>
                                                <button
                                                    onClick={() => removeFromCart(item.productId)}
                                                    style={{ marginRight: '0.5rem', background: 'none', border: 'none', color: '#ef4444', cursor: 'pointer' }}
                                                >
                                                    -
                                                </button>
                                                {item.quantity}
                                                <button
                                                    onClick={() => addToCart(item as CartItem)}
                                                    style={{ marginLeft: '0.5rem', background: 'none', border: 'none', color: '#10b981', cursor: 'pointer' }}
                                                >
                                                    +
                                                </button>
                                            </td>
                                            <td style={{ padding: '0.75rem' }}>
                                                COP {(item.price * item.quantity).toLocaleString()}
                                            </td>
                                            <td style={{ padding: '0.75rem' }}>
                                                <button
                                                    onClick={() => removeFromCart(item.productId)}
                                                    style={{ background: 'none', border: 'none', color: '#ef4444', cursor: 'pointer' }}
                                                >
                                                    Quitar
                                                </button>
                                            </td>
                                        </tr>
                                    ))}
                                </tbody>
                            </table>
                        </div>

                        <div style={{
                            backgroundColor: '#f9fafb',
                            padding: '1rem',
                            borderRadius: '8px',
                            marginBottom: '1.5rem',
                        }}>
                            <p style={{ fontSize: '1.25rem', fontWeight: 'bold', marginBottom: '0.5rem' }}>
                                Total: COP {total.toLocaleString()}
                            </p>
                            <button
                                onClick={clearCart}
                                style={{
                                    width: '100%',
                                    padding: '0.75rem',
                                    backgroundColor: '#ef4444',
                                    color: 'white',
                                    border: 'none',
                                    borderRadius: '4px',
                                    fontSize: '1rem',
                                    cursor: 'pointer',
                                    marginTop: '0.5rem',
                                }}
                            >
                                Vaciar carrito
                            </button>
                        </div>
                    </>

            {/* Botón de finalizar compra */}
            {cartItems.length > 0 && (
                <div style={{
                    marginTop: '2rem',
                    padding: '1rem',
                    backgroundColor: '#d1fae5',
                    borderRadius: '8px',
                    border: '2px solid #10b981',
                }}>
                    <p style={{ fontSize: '1rem', color: '#059669', marginBottom: '0.5rem' }}>
                        Total a pagar: COP {total.toLocaleString()}
                    </p>
                    <button
                        onClick={() => alert('Compra simulada finalizada - gracias por tu compra!')}
                        style={{
                            width: '100%',
                            padding: '0.75rem',
                            backgroundColor: '#10b981',
                            color: 'white',
                            border: 'none',
                            borderRadius: '4px',
                            fontSize: '1rem',
                            cursor: 'pointer',
                        }}
                    >
                        Finalizar compra
                    </button>
                </div>
            )}
            </>

            <p style={{ marginTop: '2rem', color: '#6b7280', fontSize: '0.875rem' }}>
                {user ? 'Usuario autenticado' : 'Inicia sesión para guardar tu carrito'}
            </p>
        </div>
    );
}