
(define *flow-graph* '())

(define-class <Node> ()
  ((name :init-value "" :init-keyword :name :accessor name-of)
   (succ :init-value '() :init-keyword :succ :accessor succ-of)
   (prev :init-value '() :init-keyword :prev :accessor prev-of)
   (use :init-value '() :init-keyword :use :accessor use-of)
   (def :init-value '() :init-keyword :def :accessor def-of)))


(define init-nodes
  (lambda ()
    (vector (make <Node> :name "1" :succ '(2) :prev '()    :def '(a))
          (make <Node> :name "2" :succ '(3) :prev '(1 5) :use '(a) :def '(b))
          (make <Node> :name "3" :succ '(4) :prev '(2)   :use '(b c)  :def '(c))
          (make <Node> :name "4" :succ '(5) :prev '(3)   :use '(b) :def '(a))
          (make <Node> :name "5" :succ '(2 6) :prev '(4) :use '(a) )
          (make <Node> :name "6" :succ '() :prev '(5) :use '(c))
          )))

(define print-node
  (lambda (n)
    (print "name: " (name-of n) " succ " (succ-of n) " prev " (prev-of n))))


(define print-graph
  (lambda (g)
    (vector-for-each print-node g)))


(set! *flow-graph* (init-nodes))

(print-graph *flow-graph*)

(define node0 '())
(set! node0 (make <Node> :name "1" :succ '(2) :prev '() :def '(a)))
(name-of node0)

