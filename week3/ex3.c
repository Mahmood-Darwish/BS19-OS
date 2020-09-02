#include <stdio.h>
#include <stdlib.h>

struct LinkedNode {
    int x;
    struct LinkedNode *next;
};

void print_list(struct LinkedNode *current) {
    while (current != NULL) {
        printf("%d, ", current->x);
        current = current->next;
    }
}

void insert_node(int index, int value, struct LinkedNode **current) {
    struct LinkedNode *new_node = (struct LinkedNode *) malloc(sizeof(struct LinkedNode));
    new_node->x = value;
    new_node->next = NULL;

    struct LinkedNode *last = *current;

    if (*current == NULL) {
        *current = new_node;
        return;
    }

    int i = 0;
    while (last->next != NULL && ++i < index) {
        last = last->next;
    }
    new_node->next = last->next;
    last->next = new_node;
}


void delete_node(int index, struct LinkedNode **current) {
    if (*current == NULL) return;
    struct LinkedNode *last = *current;

    if (index == 0) {
        *current = last->next;
        free(last);
        return;
    }

    int i = 0;
    while (last->next != NULL && ++i < index) {
        last = last->next;
    }

    struct LinkedNode *next_to_the_one_to_be_deleted = last->next->next;
    free(last->next);
    last->next = next_to_the_one_to_be_deleted;
}


int main() {
    struct LinkedNode *head = (struct LinkedNode *) malloc(sizeof(struct LinkedNode));
    head->x = 1;
    head->next = NULL;

    print_list(head->next);

    printf("\n");

    insert_node(1, 3, &head);

    print_list(head->next);

    printf("\n");

    insert_node(2, 5, &head);

    print_list(head->next);

    printf("\n");

    insert_node(1, 2, &head);

    print_list(head->next);

    printf("\n");

    insert_node(3, 4, &head);

    print_list(head->next);

    printf("\n");

    delete_node(0, &head);

    print_list(head->next);
    
    printf("\n");

    return 0;
}
